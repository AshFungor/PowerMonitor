from typing import Optional, List

from pydantic import BaseModel

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

    def verify_password(self):
        db_encrypted_password = database.select_user_by_login(self.login)[2]
        return check_password(db_encrypted_password, self.password)

    def verify_admin(self):
        is_admin = database.select_user_by_login(self.login)[3]
        return is_admin
    
    @property
    def encrypted_password(self):
        return encrypt_password(self.password)

    @property
    def request_json(self):
        return self.dict(exclude={'decrypt_password'})
