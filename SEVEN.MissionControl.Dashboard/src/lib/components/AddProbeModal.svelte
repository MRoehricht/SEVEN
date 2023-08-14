<script lang="ts">
	import type { Probe } from '$lib/types';
	import { Modal, TextInput, MultiSelect } from 'carbon-components-svelte';
	import { env } from '$env/dynamic/public';

	export let isOpen: boolean;
	export let onSubmitClicked: () => void;
	export let selectedProbe: Probe | null;

	let name = '';
	let measurementsType = 0;
	let multiSelectLabel: string;
	let multiselectIds: Array<string> = [];

	let modalHeading = 'Neues Sonde';
	let primaryButtonText = 'Sonde erstellen';

	const items = [
		{ id: '0', text: 'Ohne', flag: 0 },
		{ id: '1', text: 'Temperatur', flag: 1 },
		{ id: '2', text: 'Prozent', flag: 2 },
		{ id: '4', text: 'Ladezustand', flag: 4 },
		{ id: '8', text: 'Feuchtigkeit', flag: 8 },
		{ id: '16', text: 'UV-Strahlung', flag: 16 },
		{ id: '32', text: 'Lichtintensität', flag: 32 },
		{ id: '64', text: 'Bodenfeuchtigkeit', flag: 64 },
		{ id: '128', text: 'Schaltzustand', flag: 128 }
	];

	const formatSelected = (i: any) =>
		i.length === 0
			? 'Wähle Messwerte'
			: i.map((id: string) => items.find((item) => item.id === id)?.text).join(', ');

	$: {
		multiSelectLabel = formatSelected(multiselectIds);
		measurementsType = 0;
		multiselectIds.forEach((id) => {
			measurementsType = measurementsType + Number(id);
		});
	}

	async function createProbe(): Promise<Probe> {
		if (selectedProbe == null || selectedProbe.id.length == 0) {
			const options: RequestInit = {
				method: 'POST',
				headers: { 'Content-Type': 'application/json', accept: '*/*' },
				body: JSON.stringify({
					name: name,
					measurementsType: Number(measurementsType)
				})
			};

			return await fetch(`${env.PUBLIC_API_URL}/probe`, options).then((res) => res.json());
		} else {
			const options: RequestInit = {
				method: 'PUT',
				headers: { 'Content-Type': 'application/json', accept: '*/*' },
				body: JSON.stringify({
					id: selectedProbe.id,
					name: name,
					measurementsType: Number(measurementsType)
				})
			};

			return await fetch(`${env.PUBLIC_API_URL}/probe`, options).then((res) => res.json());
		}
	}
</script>

<Modal
	bind:open={isOpen}
	{modalHeading}
	{primaryButtonText}
	secondaryButtonText="Abbrechen"
	preventCloseOnClickOutside
	on:click:button--secondary={() => {
		isOpen = false;
	}}
	on:open={() => {
		name = selectedProbe != null ? selectedProbe.name : '';
		measurementsType = selectedProbe != null ? selectedProbe.measurementsType : 0;
		modalHeading = selectedProbe != null ? 'Sonde bearbeiten' : 'Neues Sonde';
		primaryButtonText = selectedProbe != null ? 'Sonde speichern' : 'Sonde erstellen';
		multiselectIds = [];
		for (var bit = 8; bit >= 0; bit--) {
			var mask = Math.pow(2, bit);
			if ((measurementsType & mask) == mask) {
				multiselectIds.push(Math.pow(2, bit).toString());
			}
		}
	}}
	on:close={() => {
		isOpen = false;
		name = '';
		measurementsType = 0;
		multiselectIds = [];
		selectedProbe = null;
	}}
	on:submit={async () => {
		await createProbe();
		onSubmitClicked();
		isOpen = false;
		name = '';
		measurementsType = 0;
		multiselectIds = [];
		selectedProbe = null;
	}}
>
	<TextInput id="probe-name" labelText="Sondenname" placeholder="Sondenname..." bind:value={name} />
	<MultiSelect
		titleText="Messwerte"
		label={multiSelectLabel}
		bind:selectedIds={multiselectIds}
		{items}
	/>
</Modal>

<style>
	/* Hack bezüglich https://github.com/carbon-design-system/carbon-components-svelte/issues/1619 */
	:global(.bx--modal-content) {
		overflow: visible;
	}

	:global(.bx--modal.is-visible .bx--modal-container) {
		overflow: visible;
	}
</style>
