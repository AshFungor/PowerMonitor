import unittest
from server.utils.date_parser import parse_date, date_to_str
from datetime import datetime


class DateParserTest(unittest.TestCase):

    def test_date_to_str(self):
        self.assertEqual(date_to_str(datetime(2000, 12, 24, 3, 46, 35)), "24.12.2000  03:46:35")
        self.assertEqual(date_to_str(datetime(6000, 10, 15, 23, 45, 55)), "15.10.6000  23:45:55")
        self.assertEqual(date_to_str(datetime(100, 2, 3, 19, 47, 33)), "03.02.100  19:47:33")

    def test_date_to_str_exceptions(self):
        with self.assertRaises(ValueError):
            x = date_to_str(datetime(2000, 13, 29, 3, 46, 35))

        with self.assertRaises(ValueError):
            x = date_to_str(datetime(2000, 12, 29, 29, 46, 35))

    def test_parse_date(self):
        self.assertEqual(parse_date("24.12.2000  03:46:35"), datetime(2000, 12, 24, 3, 46, 35))
        self.assertEqual(parse_date("15.10.6000  23:45:55"), datetime(6000, 10, 15, 23, 45, 55))
        self.assertEqual(parse_date("03.02.1000  19:47:33"), datetime(1000, 2, 3, 19, 47, 33))

    def test_parse_date_exceptions(self):
        with self.assertRaises(ValueError):
            x = parse_date("29.13.2000  03:46:35")

        with self.assertRaises(ValueError):
            x = parse_date("29.12.2000  29:46:35")


if __name__ == "__main__":
    unittest.main()
