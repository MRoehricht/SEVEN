import { measurementTypeLabels, type Probe } from '$lib/types';
import { FlaggedEnum } from '$lib/utils/flagged-enum';
import { env } from '$env/dynamic/public';

const flags = new FlaggedEnum(measurementTypeLabels);

export class DeviceService{
    static async fetchDevices(): Promise<Probe[]> {           
        const options: RequestInit = {
            method: 'GET',
            headers: { 'Content-Type': 'application/json', accept: '*/*' }
        };    
            
        return await fetch(`${env.PUBLIC_API_URL}/probe`, options).then((response) => response.json());
    }

    static async fetchDevice(id: string): Promise<Probe | null> {     
        if (id == null || id.length == 0) return null;          
        const options: RequestInit = {
            method: 'GET',
            headers: { 'Content-Type': 'application/json', accept: '*/*' }
        };   

        return await fetch(`${env.PUBLIC_API_URL}/probe/${id}`, options).then((response) => response.json());
    }

    static async createDevice(name: string, selectedIds: string[]): Promise<Probe>{
        const options: RequestInit = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', accept: '*/*' },
            body: JSON.stringify({
                name: name,
                measurementsType: flags.getValueFromIds(selectedIds)
            })
        };
       
        const probe = await fetch(`${env.PUBLIC_API_URL}/probe`, options).then((res) => res.json());
        document.dispatchEvent(new CustomEvent('navigationUpdated'));
        return probe;
    }

    static async updateDevice(probeId: string, name: string, selectedIds: string[]): Promise<Probe>{       
        const options: RequestInit = {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', accept: '*/*' },
            body: JSON.stringify({
                id: probeId,
                name: name,
                measurementsType: flags.getValueFromIds(selectedIds)
            })
        };
      
        const probe = await fetch(`${env.PUBLIC_API_URL}/probe`, options).then((res) => res.json());
        document.dispatchEvent(new CustomEvent('navigationUpdated'));
        return probe;
    }

    static async deleteDevice(id: string) {
		if (id == null || id.length == 0) return;

		const options: RequestInit = {
			method: 'DELETE',
			headers: { 'Content-Type': 'application/json', accept: '*/*' }
		};

		await fetch(`${env.PUBLIC_API_URL}/probe?id=` + id, options);
		document.dispatchEvent(new CustomEvent('navigationUpdated'));
	}
}