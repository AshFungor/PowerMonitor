import unittest
from utils.request_parser import parse_user_info, parse_admin_info
import json

"""
username -> any
password -> any
is_admin -> bool
complexes -> [n*int] 
"""


class TestRequestParser(unittest.TestCase):
    def test_parse_user_info(self):
        with open("examples/create-user-test.json", "r") as read_file:
            data = json.load(read_file)
            user = data['user']
            complexes = user['complexes']
            indications = (user['login'], user['password'], user['is_admin'])
            self.assertEqual(parse_user_info(data), {'user': indications, 'complexes': complexes})

        with open("examples/create-user-test2.json", "r") as read_file:
            data = json.load(read_file)
            user = data['user']
            complexes = user['complexes']
            indications = (user['login'], user['password'], user['is_admin'])
            self.assertEqual(parse_user_info(data), {'user': indications, 'complexes': complexes})

    def test_parse_admin_info(self):
        with open("examples/create-user-test.json", "r") as read_file:
            data = json.load(read_file)
            admin = data['admin']
            self.assertEqual(parse_admin_info(data), (admin['login'], admin['password']))

        with open("examples/create-user-test2.json", "r") as read_file:
            data = json.load(read_file)
            admin = data['admin']
            self.assertEqual(parse_admin_info(data), (admin['login'], admin['password']))
