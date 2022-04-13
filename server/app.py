from flask import Flask, request, Response, jsonify
from werkzeug.exceptions import HTTPException

from utils.csv_api.csv_converter import parse_csv, create_csv
from utils.database_api.models import User, Telemetry, Complex, Admin
from utils.date_parser import parse_date
from db_loader import database

app = Flask(__name__)


@app.route('/', methods=['GET'])
def init_db():
    database.initiate()
    return Response(status=200)


@app.route('/get-data', methods=['GET'])
def get_data():
    """
    Returns telemetry by get-data request.

    Request has to include json file (see get-data.json in request_examples)
    """
    request_ = request.json
    user = User.parse_obj(request_['user'])
    user.verify()
    start = parse_date(request_['start'])
    end = parse_date(request_['end'])
    telemetry = database.select_telemetry(start, end, user)
    csv_response = create_csv(telemetry)
    return Response(csv_response, mimetype='text/csv', status=200)


@app.route('/get-user-info', methods=['GET'])
def get_user_info():
    request_ = request.json
    user = User.parse_obj(request_['user'])
    user.verify()
    complexes = database.select_complexes_by_user_login(user.login)
    is_admin = database.select_user_by_login(user.login)[3]
    user_info = jsonify({
        'is_admin': is_admin,
        'complexes': complexes,
    })
    return user_info


@app.route('/get-all-users', methods=['GET'])
def get_all_users():
    """
    Returns information about each user in json format.

    *Requires admin permission.*
    """
    request_ = request.json
    admin = Admin.parse_obj(request_['admin'])
    admin.verify()
    users = []
    for record in database.select_all_users():
        login, password, is_admin = record
        complexes = database.select_complexes_by_user_login(login)
        user = User(
            login=login,
            password=password,
            is_admin=is_admin,
            complex_ids=complexes,
            decrypt_password=True
        )
        users.append(user.request_json)
    response = jsonify(users)
    return response


@app.route('/send-telemetry/<int:serial_number>', methods=['POST'])
def send_telemetry(serial_number: int):
    """
    Receives telemetry from complex and inserts it to database.

    Requires a serial number in request url.

    param serial_number: int
    """
    request_ = request.files['table']
    csv_file = request_
    table = parse_csv(csv_file)
    table = [Telemetry(serial_number, *measurements) for measurements in table]
    for telemetry in table:
        database.add_telemetry(telemetry)
    return Response(status=200)


@app.route('/delete-user', methods=['POST', 'DELETE'])
def delete_user():
    """
    Deletes user by delete-user request.

    *Requires admin permission.*
    """
    request_ = request.json
    admin = Admin.parse_obj(request_['admin'])
    admin.verify()
    user_login = request_['user_login']
    database.delete_user_by_login(user_login)
    database.delete_complexes_by_user_login(user_login)
    return Response(status=200)


@app.route('/create-user', methods=['POST'])
def create_user():
    """
    Creates user by create-user request.

    *Requires admin permission.*
    """
    request_ = request.json
    admin = Admin.parse_obj(request_['admin'])
    admin.verify()
    user = User.parse_obj(request_['user'])
    for serial_number in request_['user'].get('complexes', []):
        complex_ = Complex(user_login=user.login, serial_number=serial_number)
        database.add_complex(complex_)
    database.add_user(user)
    return Response(status=200)


@app.route('/bind-complex', methods=['POST', 'PUT'])
def bind_complex():
    """
    Binds complexes to user.

    *Requires admin permission.*
    """
    request_ = request.json
    admin = Admin.parse_obj(request_['admin'])
    admin.verify()
    serials = request_['complexes']
    user_login = request_['user_login']
    for serial in serials:
        complex_ = Complex(
            user_login=user_login,
            serial_number=serial
        )
        database.add_complex(complex_)
    return Response(status=200)


@app.errorhandler(HTTPException)
def handle_http_exception(e: HTTPException):
    """Returns JSON response for HTTP errors"""
    response = e.get_response()
    response.data = jsonify({
        "code": e.code,
        "name": e.name,
        "description": e.description,
    }).data
    response.mimetype = "application/json"
    return response


if __name__ == '__main__':
    app.run()
