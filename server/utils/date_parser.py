from datetime import datetime
import re


def parse_date(date):
    if re.match(r'\d{2}\.\d{2}\.\d{4}\s+\d{2}:\d{2}:\d{2}', date):
        parsed_date = datetime.strptime(date, '%d.%m.%Y  %X')
    else:
        parsed_date = datetime.strptime(date, '%d.%m.%Y')
    return parsed_date


def date_to_str(date: datetime):
    return date.strftime('%d.%m.%Y  %X')
