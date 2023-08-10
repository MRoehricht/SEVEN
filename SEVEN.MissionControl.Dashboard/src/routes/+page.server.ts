import { env } from '$env/dynamic/private';

/** @type {import('./$types').PageLoad} */
export async function load({ fetch, params }) {
	async function fetchData(): Promise<Measurement[]> {
		const response = await fetch(`${env.PUBLIC_}/measurement`);
		return (await response.json()) as Measurement[];
	}

	let measurements = [] as Measurement[];
	return { measurements };
}

export type Measurement = {
	id: string;
	probeId: string;
	measurementType: number;
	value: string;
	time: string;
	localTime: string;
};
