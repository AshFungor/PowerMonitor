from flask import Flask, make_response, request

from server.utils.database_api.database import Database
from server.data.config import NAME, USER, PASSWORD, HOST
from server.utils.csv_api.csv_parser import parse_csv
from server.utils.request_parser import parse_user_info
from server.utils.database_api.tables import User, Telemetry, Complex

app = Flask(__name__)
database = Database(NAME, USER, PASSWORD, HOST)


@app.route('/get-data', methods=['GET'])
def get_data():
    ...
    # json = request.json
    # request_ = parse_user_info(json)
    # user = User(*request_)
    # response = make_response(send_file(''))
    # response.headers["Content-type"] = "text/csv"
    # response.status_code = 200
    # return response


@app.route('/create-user', methods=['POST'])
def create_user():
    request_ = parse_user_info(request.json)
    user = User(*request_['user'])
    for serial_number in request_['complexes']:
        complex = Complex(user.login, serial_number)
        database.add_complex(complex)
    database.add_user(user)
    response = make_response()
    response.status_code = 200
    return response


@app.route('/send-telemetry/<int:serial_number>', methods=['POST'])
def send_telemetry(serial_number: int):
    request_ = request.files['table']
    csv_file = request_
    table = parse_csv(csv_file)
    table = [Telemetry(serial_number, *measurements) for measurements in table]
    for telemetry in table:
        database.add_telemetry(telemetry)
    return '200'


if __name__ == '__main__':
    database.initiate()
    app.run()
