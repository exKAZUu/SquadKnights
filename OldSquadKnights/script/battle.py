# -*- coding: utf-8 -*-

from System import Random

random = Random()

def calc_porb(attacker, defender, act):
    # 特に指定がなければ確実に命中
    prob = 1000
    if act.ActKind == "斧":
        prob = 70
    elif act.ActKind == "槍":
        prob = 75
    elif act.ActKind == "弓":
        prob = 70
    elif act.ActKind == "魔":
        prob = 75
    elif act.ActKind == "剣":
        prob = 80
    return prob + attacker.Skl * 2 - defender.Agi

def calc_effect(attacker, defender, act):
    # 種類に応じたダメージ計算
    if act.ActKind == "斧":
        return int(attacker.Atk * 4 + attacker.Skl - defender.Def * 2)
    elif act.ActKind == "槍":
        return int(attacker.Atk * 3 + attacker.Skl * 2 - defender.Def * 2)
    elif act.ActKind == "弓":
        return int(attacker.Atk * 3 + attacker.Skl * 2 - defender.Def * 2)
    elif act.ActKind == "魔":
        return int(attacker.Atk * 3.5)
    elif act.ActKind == "剣":
        return int(attacker.Atk * 2.5 + attacker.Skl *2.5 - defender.Def * 2)
    elif act.ActKind == "薬草":
        return int(50)
    elif act.ActKind == "範囲魔法":
        return int(attacker.Atk * 2.5)