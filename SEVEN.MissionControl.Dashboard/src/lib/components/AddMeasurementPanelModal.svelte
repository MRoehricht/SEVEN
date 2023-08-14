<script lang="ts">
	import { measurementTypeLabels, type AddMeasurementPanel } from '$lib/types';
	import { FlaggedEnum } from '$lib/utils/flagged-enum';
	import { Modal, TextInput, Select, SelectItem } from 'carbon-components-svelte';

	let title = '';
	let probeId = '';
	let refreshInterval = 0;

	let measurementType = 0;
	const flags = new FlaggedEnum(measurementTypeLabels);
	let flagsWithLabels = flags.getAllValuesWithLabels();

	export let isOpen: boolean;
	export let onSubmitClicked: (panel: AddMeasurementPanel) => void;
	export let selectedPanel: AddMeasurementPanel | null;
</script>

<Modal
	bind:open={isOpen}
	modalHeading="Neues Panel"
	primaryButtonText="Panel erstellen"
	secondaryButtonText="Abbrechen"
	on:click:button--secondary={() => (isOpen = false)}
	on:open={() => {
		if (selectedPanel) {
			title = selectedPanel.title;
			probeId = selectedPanel.probeId;
			refreshInterval = selectedPanel.refreshInterval;
			measurementType = selectedPanel.measurementType;
		}
	}}
	on:close={() => {
		isOpen = false;
		title = '';
		probeId = '';
		refreshInterval = 0;
		measurementType = 0;
		selectedPanel = null;
	}}
	on:submit={() => {
		onSubmitClicked({
			id: selectedPanel?.id ?? '',
			title,
			probeId,
			measurementType,
			refreshInterval
		});

		isOpen = false;
		title = '';
		probeId = '';
		refreshInterval = 0;
		measurementType = 0;
	}}
>
	<TextInput id="panel-name" labelText="Panelname" placeholder="Panelname..." bind:value={title} />
	<TextInput id="probe-id" labelText="Probe-Id" placeholder="Probe-Id..." bind:value={probeId} />
	<Select labelText="Measurement-Typ" bind:selected={measurementType}>
		{#each flagsWithLabels as [value, label]}
			<SelectItem {value} text={label} />
		{/each}
	</Select>
	<TextInput
		id="refresh-interval"
		labelText="Refresh-Interval (in Sekunden)"
		placeholder="Refresh-Interval..."
		bind:value={refreshInterval}
	/>
</Modal>
