import io
import csv

from utils.date_parser import parse_date, date_to_str


def parse_row(row):
    record = row.decode('utf-8').strip().split(';')
    converting_functions = [parse_date] * 2 + [float] * 24 + [int]
    record = [function(value) if value != '' else None
              for value, function in zip(record, converting_functions)]
    return record


def parse_csv(csv_file):
    """declare csv file in to convert csv-readable format into database-readable format"""
    table = [parse_row(row) for row in csv_file.readlines()]
    return table


def create_csv(rows):
    """generates a csv file based on database records"""
    records = [row[2:] for row in rows]
    converting_functions = [date_to_str] * 2 + [str] * 25
    csv_file = io.StringIO()
    writer = csv.writer(csv_file, delimiter=';')
    for record in records:
        writer.writerow(function(value) if value is not None else ''
                        for value, function in zip(record, converting_functions))
    csv_file.seek(0)
    return csv_file
