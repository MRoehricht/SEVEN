export type Measurement = {
	id: string;
	probeId: string;
	measurementType: number;
	value: string;
	time: string;
	localTime: string;
};

export type AddMeasurementPanel = {
	title: string;
	probeId: string;
	measurementType: number;
};

export type Probe = {
	id: string;
	name: string;	
	measurementsType: number;
	sendingIntervalMinutes: number;
	lastContact: string;
};
