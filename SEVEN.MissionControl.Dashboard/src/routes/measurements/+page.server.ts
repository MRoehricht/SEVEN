import { API_URL } from '$env/static/private';

/** @type {import('./$types').PageLoad} */
export async function load({ fetch, params }) {
	async function fetchData(): Promise<Measurement[]> {
		const response = await fetch(`${API_URL}/measurement`);
		return (await response.json()) as Measurement[];
	}

	let measurements = await fetchData();
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
