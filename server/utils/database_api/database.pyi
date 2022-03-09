from typing import Union


class Database:
    def __init__(self, dbname: str, user: str, password: str, host: str) -> None:
        self.name = dbname
        self.user = user
        self.password = password
        self.host = host

    @property
    def connection(self): ...

    def execute(self, sql: str, parameters: Union[tuple, dict, list] = None, fetchone: bool = False,
                fetchall: bool = False, commit: bool = False) -> Union[tuple, None]: ...

    def add_user(self, login: str, password: str, is_admin: bool) -> None: ...

    def select_user_by_login(self, login: str) -> tuple: ...
