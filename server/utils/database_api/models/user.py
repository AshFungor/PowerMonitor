from typing import Optional, List

from pydantic import BaseModel, ValidationError
from werkzeug.exceptions import Forbidden, BadRequest

from db_loader import database
from security.encryption import encrypt_password, check_password, decrypt_password


class User(BaseModel):
    login: str
    password: str
    is_admin: bool = False
    complex_ids: Optional[List[int]] = []
    decrypt_password: bool = False

    def __init__(self, **data):
        try:
            super().__init__(**data)
            if self.decrypt_password:
                self.password = decrypt_password(self.password)
        except ValidationError as e:
            print(e.errors())
            errors = ' // '.join(f"location: {', '.join(error['loc'])}; error: {error['msg']}"
                               for error in e.errors())
            print(errors)
            raise BadRequest(f'VALIDATION ERROR // '
                             f'{errors}')

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
