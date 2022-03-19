from werkzeug.security import generate_password_hash, check_password_hash
from pydantic import BaseModel

from utils.database_api.database import Database


class User(BaseModel):
    login: str
    password: str
    is_admin: bool = False

    def __init__(self, **data):
        super().__init__(**data)
        self.password = generate_password_hash(self.password)

    def verify_password(self, database: Database):
        db_password_hash = database.select_user_by_login(self.login)[2]
        return check_password_hash(db_password_hash, self.password)

    def verify_admin(self, database: Database):
        is_admin = database.select_user_by_login(self.login)[3]
        return is_admin
