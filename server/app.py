import json

from flask import Flask, request, Response

from data.config import NAME, USER, PASSWORD, HOST
from utils.database_api.database import Database
from utils.csv_api.csv_converter import parse_csv, create_csv
from utils.database_api.models import User, Telemetry, Complex
from utils.date_parser import parse_date


app = Flask(__name__)
database = Database(NAME, USER, PASSWORD, HOST)


@app.route('/get-data', methods=['GET'])
def get_data():
    json = request.json
    start = parse_date(json['start'])
    end = parse_date(json['end'])
    telemetry = database.select_telemetry_by_date(start, end)
    csv_response = create_csv(telemetry)
    return Response(csv_response, mimetype='text/csv', status=200)


@app.route('/create-user', methods=['POST'])
def create_user():
    request_ = request.json
    user = User.parse_obj(request_['user'])
    for serial_number in request_['user']['complexes']:
        complex = Complex(user_login=user.login, serial_number=serial_number)
        database.add_complex(complex)
    database.add_user(user)
    return Response(status=200)


@app.route('/send-telemetry/<int:serial_number>', methods=['POST'])
def send_telemetry(serial_number: int):
    request_ = request.files['table']
    csv_file = request_
    table = parse_csv(csv_file)
    table = [Telemetry(serial_number, *measurements) for measurements in table]
    for telemetry in table:
        database.add_telemetry(telemetry)
    return Response(status=200)


@app.route('/get-all-users', methods=['GET'])
def get_all_users():
    users = dict()
    for i, record in enumerate(database.select_all_users(), start=1):
        login, password, is_admin = record
        complexes = tuple(map(lambda x: x[0], database.select_complexes_by_user_login(login)))
        user = User(
            login=login,
            password=password,
            is_admin=is_admin,
            complex_ids=complexes,
            decrypt_password=True
        )
        users[f'user-{i}'] = json.loads(user.json(exclude={'decrypt_password'}))
    return users


if __name__ == '__main__':
    database.initiate()
    app.run()
