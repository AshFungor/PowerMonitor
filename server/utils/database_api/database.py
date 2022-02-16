import sqlite3


class Database:
    def __init__(self, path_to_db='database.db'):
        self.path_to_db = path_to_db

    @property
    def connection(self):
        return sqlite3.connect(self.path_to_db)

    def execute(self, sql: str, parameters: tuple = tuple(), fetchone=False, fetchall=False, commit=False):
        connection = self.connection
        cursor = connection.cursor()
        data = None
        cursor.execute(sql, parameters)
        if commit:
            connection.commit()
        if fetchone:
            data = cursor.fetchone()
        if fetchall:
            data = cursor.fetchall()

        return data

    def create_table_users(self):
        sql = """
        CREATE TABLE IF NOT EXISTS
        users (
        id integer NOT NULL,
        name Varchar(255) NOT NULL,
        PRIMARY KEY (id)
        );
        """
        self.execute(sql, commit=True)

    def add_user(self, id: int, name: str):
        sql = """
        INSERT INTO users(id, name) VALUES(?, ?)
        """
        self.execute(sql, parameters=(id, name), commit=True)

    def select_all_users(self):
        sql = "SELECT * FROM users"
        return self.execute(sql, fetchall=True)
