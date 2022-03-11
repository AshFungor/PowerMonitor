from dataclasses import dataclass
from typing import Optional
from datetime import datetime


@dataclass
class Telemetry:
    serial_number: int
    start: datetime
    finish: datetime
    reactive_power_by_phase_A: float
    reactive_power_by_phase_B: float
    reactive_power_by_phase_C: float
    active_power_by_phase_A: float
    active_power_by_phase_B: float
    active_power_by_phase_C: float
    voltage_by_phase_A: float
    voltage_by_phase_B: float
    voltage_by_phase_C: float
    cos_by_phase_A: float
    cos_by_phase_B: float
    cos_by_phase_C: float
    reactive_power_by_phase_A_off: Optional[float]
    reactive_power_by_phase_B_off: Optional[float]
    reactive_power_by_phase_C_off: Optional[float]
    active_power_by_phase_A_off: Optional[float]
    active_power_by_phase_B_off: Optional[float]
    active_power_by_phase_C_off: Optional[float]
    voltage_by_phase_A_off: Optional[float]
    voltage_by_phase_B_off: Optional[float]
    voltage_by_phase_C_off: Optional[float]
    cos_by_phase_A_off: Optional[float]
    cos_by_phase_B_off: Optional[float]
    cos_by_phase_C_off: Optional[float]
    number_of_enabled_blocks: int

    @property
    def parameter_names(self) -> tuple:
        return tuple(self.__dict__.keys())

    @property
    def parameters(self) -> tuple:
        return tuple(self.__dict__.values())
