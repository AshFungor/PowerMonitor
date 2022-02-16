from flask import Flask, send_file
from utils.database_api.database import Database


app = Flask(__name__)
database = Database()


@app.route('/<int:serial_number>/<string:file_name>', methods=['GET'])
def send_data(serial_number: int, file_name: str):
    return send_file(f'PredprofExamples/{file_name}.csv')


@app.route('/<int:id>/<string:name>')
def create_user(id: int, name: str):
    database.add_user(id, name)
    return


if __name__ == '__main__':
    app.run()