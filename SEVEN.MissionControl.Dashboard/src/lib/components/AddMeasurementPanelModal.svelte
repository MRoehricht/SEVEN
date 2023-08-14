<script lang="ts">
	import { measurementTypeLabels, type AddMeasurementPanel } from '$lib/types';
	import { FlaggedEnum } from '$lib/utils/flagged-enum';
	import { Modal, TextInput, ComboBox, Select, SelectItem } from 'carbon-components-svelte';

	let title = '';
	let probeId = '';

	let measurementType = 0;
	const flags = new FlaggedEnum(measurementTypeLabels);
	let flagsWithLabels = flags.getAllValuesWithLabels();

	export let isOpen: boolean;
	export let onSubmitClicked: (panel: AddMeasurementPanel) => void;
</script>

<Modal
	bind:open={isOpen}
	modalHeading="Neues Panel"
	primaryButtonText="Panel erstellen"
	secondaryButtonText="Abbrechen"
	on:click:button--secondary={() => (isOpen = false)}
	on:open
	on:close
	on:submit={() => {
		onSubmitClicked({
			title,
			probeId,
			measurementType
		});

		isOpen = false;
		title = '';
		probeId = '';
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
</Modal>
