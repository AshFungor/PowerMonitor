from typing import Optional, List

from pydantic import BaseModel
from werkzeug.exceptions import Forbidden

from db_loader import database
from security.encryption import encrypt_password, check_password, decrypt_password


class User(BaseModel):
    login: str
    password: str
    is_admin: bool = False
    complex_ids: Optional[List[int]] = []
    decrypt_password: bool = False

    def __init__(self, **data):
        super().__init__(**data)
        if self.decrypt_password:
            self.password = decrypt_password(self.password)

    def verify(self):
        user_data = database.select_user_by_login(self.login)
        if not user_data:
            raise Forbidden('User not found')
        db_encrypted_password = user_data[2]
        db_admin_permission = user_data[3]
        if not check_password(db_encrypted_password, self.password):
            raise Forbidden('Incorrect password')
        if isinstance(self, Admin) and not db_admin_permission:
            raise Forbidden('Administrator rights required')

    @property
    def encrypted_password(self):
        return encrypt_password(self.password)

    @property
    def request_json(self):
        return self.dict(exclude={'decrypt_password'})


class Admin(User):
    pass
