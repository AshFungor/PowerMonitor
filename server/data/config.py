import os
from dotenv import load_dotenv

load_dotenv()
keys = ['NAME', 'USER', 'PASSWORD', 'HOST']

NAME, USER, PASSWORD, HOST = [str(os.getenv(key)) for key in keys]
