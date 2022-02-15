from flask import Flask, send_file

app = Flask(__name__)


@app.route('/get', methods=['GET'])
def value():
    return send_file('PredprofExamples/1607553426.csv')


if __name__ == '__main__':
    app.run()