from flask import Flask, send_file
from server.utils.database_api.database import Database
from server.data.config import NAME, USER, PASSWORD, HOST


app = Flask(__name__)
database = Database(NAME, USER, PASSWORD, HOST)


@app.route('/<int:serial_number>/<string:file_name>', methods=['GET'])
def send_data(serial_number: int, file_name: str):
    return send_file(f'PredprofExamples/{file_name}.csv')


@app.route('/create-user/<string:login>/<string:password>/<int:is_admin>')
def create_user(login: str, password: str, is_admin: int):
    is_admin = bool(is_admin)
    database.add_user(login, password, is_admin)
    return


if __name__ == '__main__':
    database.initiate()
    app.run()
