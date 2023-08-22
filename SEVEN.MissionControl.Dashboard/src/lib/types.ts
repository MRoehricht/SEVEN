export type Measurement = {
	id: string;
	probeId: string;
	measurementType: number;
	value: string;
	time: string;
	localTime: string;
};

export type MeasurementWithGroup = Measurement & { group: string };

export type AddMeasurementPanel = {
	id: string;
	title: string;
	probeId: string;
	refreshInterval: number;
	measurementType: number;
	showCurrentValue: boolean;
};

export type Probe = {
	id: string;
	name: string;
	measurementsType: number;
	sendingIntervalMinutes: number;
	lastContact: string;
};

export enum MeasurementType {
	Without = 0,
	Temperature = 1,
	Percent = 2,
	StateOfCharge = 4,
	Humidity = 8,
	UvRadiation = 16,
	LightIntensity = 32,
	SoilMoisture = 64,
	SwitchingState = 128,
	AirPressure = 256
}

export const measurementTypeLabels: Record<MeasurementType, string> = {
	[MeasurementType.Without]: 'Without',
	[MeasurementType.Temperature]: 'Temperature',
	[MeasurementType.Percent]: 'Percent',
	[MeasurementType.StateOfCharge]: 'State of Charge',
	[MeasurementType.Humidity]: 'Humidity',
	[MeasurementType.UvRadiation]: 'UV Radiation',
	[MeasurementType.LightIntensity]: 'Light Intensity',
	[MeasurementType.SoilMoisture]: 'Soil Moisture',
	[MeasurementType.SwitchingState]: 'Switching State',
	[MeasurementType.AirPressure]: 'Air Pressure'
};

export type DqlToken = {
	type: 'property' | 'operator' | 'logical' | 'value';
	value: string;
	index: number;
};

export type DqlProperty = {
	propertyName: string;
	propertyValues: string[];
};
