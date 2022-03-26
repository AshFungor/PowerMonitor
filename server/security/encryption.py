from cryptography.fernet import Fernet
from data.config import ENCRYPTION_KEY


cryptographer = Fernet(ENCRYPTION_KEY.encode())


def encrypt_password(password: str) -> str:
    encoded = password.encode()
    encrypted_password = str(cryptographer.encrypt(encoded), 'utf-8')
    return encrypted_password


def decrypt_password(encrypted_password: str) -> str:
    encrypted_password = encrypted_password.encode()
    decrypted_password = cryptographer.decrypt(encrypted_password)
    return str(decrypted_password, 'utf-8')


def check_password(encrypted_password: str, password: str) -> bool:
    is_valid = password == decrypt_password(encrypted_password)
    return is_valid
