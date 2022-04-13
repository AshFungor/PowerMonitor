import unittest
import tempfile
import os
from app import app
from db_loader import database

"""
test_send_telemetry()
"""


class TestApp(unittest.TestCase):

    def setUp(self):
        self.tester = app.test_client()
        self.db_fd, app.config['DATABASE'] = tempfile.mkstemp()
        app.config['TESTING'] = True
        self.app = app.test_client()
        database.initiate()

    def test_empty_db(self):
        response = self.app.get('/', content_type="html/text")
        self.assertEqual(response.status_code, 200)

    def test_send_telemetry(self):
        with open('examples/1607582294.csv', 'rb') as file:
            file = {
                'table': file
            }
            request = self.tester.post('/send-telemetry/1232134', content_type="multipart/form-data", data=file)
            self.assertEqual(request.status_code, 200)

    def test_create_user(self):
        with open("examples/create-user.json") as file:
            action = self.tester.post('/create-user', content_type="application/json", data=file)
            self.assertEqual(action.status_code, 200)

    def test_delete_user(self):
        with open("examples/delete-user.json") as file:
            action = self.tester.post('/delete-user', content_type='application/json', data=file)
            self.assertEqual(action.status_code, 200)

    def test_get_data(self):
        with open("examples/get-telemetry.json") as file:
            action = self.tester.get('/get-data', content_type='application/json', data=file)
            self.assertEqual(action.status_code, 200)

    def test_get_all_users(self):
        with open("examples/get-all-users.json") as file:
            action = self.tester.get('/get-all-users', content_type="application/json", data=file)
            self.assertEqual(action.status_code, 200)

    def test_bind_complex(self):
        with open("examples/bind-complex.json") as file:
            action = self.tester.post('/bind-complex', content_type="application/json", data=file)
            self.assertEqual(action.status_code, 200)

    def tearDown(self) -> None:
        os.close(self.db_fd)
        os.unlink(app.config['DATABASE'])


if __name__ == "__main__":
    unittest.main()
