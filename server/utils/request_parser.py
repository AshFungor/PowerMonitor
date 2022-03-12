from typing import Dict, Tuple, Union, List, Optional


UserData = Dict[str, Union[str, bool, List[Optional[int]]]]
RequestData = Dict[str, UserData]
ComplexData = List[Optional[int]]
ReplyData = Dict[str, Union[Tuple[str, str, bool], ComplexData]]


def parse_user_info(json: RequestData) -> ReplyData:
    user = json['user']
    complexes = user['complexes']
    return {'user': (user['login'], user['password'], user['is_admin']),
            'complexes': complexes}


def parse_admin_info(json) -> Tuple[str, str]:
    admin = json['admin']
    return admin['login'], admin['password']
