from pydantic import BaseModel


class Complex(BaseModel):
    user_login: str
    serial_number: int
