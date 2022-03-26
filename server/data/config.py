import os
from cryptography.fernet import Fernet
from dotenv import load_dotenv, find_dotenv, set_key

dotenv_path = find_dotenv()
load_dotenv()

keys = ['ENCRYPTION_KEY', 'NAME', 'USER', 'PASSWORD', 'HOST']

if os.getenv('ENCRYPTION_KEY') is None:
    set_key(dotenv_path, 'ENCRYPTION_KEY', str(Fernet.generate_key(), 'utf-8'))
    load_dotenv()

ENCRYPTION_KEY, NAME, USER, PASSWORD, HOST = [str(os.getenv(key)) for key in keys]
