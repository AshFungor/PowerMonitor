import re
import datetime


def parse_date(date):
    regular = r'(?P<day>\d{2})\.(?P<month>\d{2})\.(?P<year>\d{4})\s+' \
              r'(?P<hour>\d{2})\:(?P<minute>\d{2})\:(?P<second>\d{2})'
    match = re.search(regular, date)
    day, month, year, hour, minute, second = map(int, match.groups())
    parsed_date = datetime.datetime(year, month, day, hour, minute, second)
    return parsed_date


def parse_row(row):
    record = row.decode('utf-8').strip().split(';')
    converting_functions = [parse_date] * 2 + [float] * 24 + [int]
    record = [function(value) if value != '' else None
              for value, function in zip(record, converting_functions)]
    return record


def parse_csv(csv_file):
    """declare the path to csv file in to convert csv-readable format into database-readable format"""
    table = [parse_row(row) for row in csv_file.readlines()]
    return table
