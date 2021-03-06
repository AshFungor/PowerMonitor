from typing import Union, Optional, Tuple
from datetime import datetime
from utils.database_api.models import User, Complex, Telemetry


class Database:
    def __init__(self, dbname: str, user: str, password: str, host: str) -> None:
        self.name = dbname
        self.user = user
        self.password = password
        self.host = host
    @property
    def connection(self): ...
    def execute(self, sql: str, parameters: Optional[Union[tuple, dict, list]] = None, fetchone: bool = False,
                fetchall: bool = False, commit: bool = False) -> Optional[tuple]: ...
    def _create_table_users(self) -> None: ...
    def _create_table_complexes(self) -> None: ...
    def _create_table_telemetry(self) -> None: ...
    def _initiate_admin(self) -> None: ...
    def initiate(self) -> None: ...
    def add_user(self, user: User) -> None: ...
    def delete_user_by_login(self, login: str) -> None: ...
    def add_telemetry(self, telemetry: Telemetry) -> None: ...
    def add_complex(self, complex: Complex) -> None: ...
    def delete_complexes_by_user_login(self, login: str) -> None: ...
    def select_user_by_login(self, login: str) -> Tuple[int, str, str, str]: ...
    def select_all_users(self) -> list: ...
    def select_complexes_by_user_login(self, login: str) -> list: ...
    def select_telemetry(self, start: datetime, end: datetime, user: User, complex_id: int) -> list: ...
