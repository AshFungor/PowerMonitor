from datetime import datetime
import re


def parse_date(date):
    try:
        if re.match(r'\d{2}\.\d{2}\.\d{4}\s+\d{2}:\d{2}:\d{2}', date):
            parsed_date = datetime.strptime(date, '%d.%m.%Y  %X')
        else:
            parsed_date = datetime.strptime(date, '%d.%m.%Y')
    except ValueError:
        raise ValueError
    return parsed_date


def date_to_str(date: datetime):
    try:
        date = date.strftime('%d.%m.%Y  %X')
    except ValueError:
        raise ValueError
    return date
