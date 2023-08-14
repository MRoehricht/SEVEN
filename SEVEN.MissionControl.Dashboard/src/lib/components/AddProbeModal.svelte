<script lang="ts">
	import { measurementTypeLabels, type Probe } from '$lib/types';
	import { Modal, TextInput, MultiSelect } from 'carbon-components-svelte';
	import { env } from '$env/dynamic/public';
	import { FlaggedEnum } from '$lib/utils/flagged-enum';

	export let isOpen: boolean;
	export let onSubmitClicked: () => void;
	export let selectedProbe: Probe | null;

	let name = '';
	let modalHeading = 'Neues Sonde';
	let primaryButtonText = 'Sonde erstellen';

	const flags = new FlaggedEnum(measurementTypeLabels);
	const flagsWithLabels = flags.getAllValuesAsIdTextObjects();
	let selectedIds: string[] = [];
	let multiSelectLabel = '';

	async function createProbe(): Promise<Probe> {
		if (selectedProbe == null || selectedProbe.id.length == 0) {
			const options: RequestInit = {
				method: 'POST',
				headers: { 'Content-Type': 'application/json', accept: '*/*' },
				body: JSON.stringify({
					name: name,
					measurementsType: flags.getValueFromIds(selectedIds)
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
					measurementsType: flags.getValueFromIds(selectedIds)
				})
			};

			return await fetch(`${env.PUBLIC_API_URL}/probe`, options).then((res) => res.json());
		}
	}

	$: {
		multiSelectLabel = flags.getLabelsFromIds(selectedIds).join(', ') || 'Wähle Messwerte';
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
		modalHeading = selectedProbe != null ? 'Sonde bearbeiten' : 'Neues Sonde';
		primaryButtonText = selectedProbe != null ? 'Sonde speichern' : 'Sonde erstellen';
		selectedIds =
			selectedProbe != null ? flags.getValuesAsStrings(selectedProbe.measurementsType) : [];
	}}
	on:close={() => {
		isOpen = false;
		selectedIds = [];
		name = '';
		multiSelectLabel = '';
	}}
	on:submit={async () => {
		await createProbe();
		onSubmitClicked();
		isOpen = false;
		selectedIds = [];
		name = '';
		multiSelectLabel = '';
	}}
>
	<TextInput id="probe-name" labelText="Sondenname" placeholder="Sondenname..." bind:value={name} />
	<MultiSelect
		titleText="Messwerte"
		label={multiSelectLabel}
		items={flagsWithLabels}
		bind:selectedIds
		sortItem={() => {}}
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
