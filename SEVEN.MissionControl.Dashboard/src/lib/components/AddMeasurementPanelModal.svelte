<script lang="ts">
	import type { AddMeasurementPanel } from '$lib/types';
	import { Modal, TextInput, ComboBox, Select, SelectItem } from 'carbon-components-svelte';

	let title = '';
	let probeId = '';
	let measurementType = 0;

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
		<SelectItem value="0" text="Temperature" />
		<SelectItem value="1" text="Percent" />
		<SelectItem value="2" text="StateOfCharge" />
		<SelectItem value="4" text="Humidity" />
		<SelectItem value="8" text="UvRadiation" />
		<SelectItem value="16" text="LightIntensity" />
		<SelectItem value="32" text="SwitchingState" />
		<SelectItem value="64" text="SoilMoisture" />
	</Select>
</Modal>
