<script lang="ts">
	import { TextInput } from 'carbon-components-svelte';

	type Token = {
		type: 'property' | 'operator' | 'logical' | 'value';
		value: string;
	};

	let properties = ['MeasurementType', 'ProbeId'];
	let propertySuggestions: Record<string, string[]> = {
		MeasurementType: ['64', '128'],
		ProbeId: ['123', '456']
	};

	let operators = ['==', '!=', '>', '<'];
	let logicals = ['AND', 'OR'];

	let tokens: Token[] = [];
	let suggestions: string[] = [];
	let value: string = '';
	let invalidInput: boolean = false;
	let invalidInputMessage: string = '';

	$: {
		tokenize(value);
		updateSuggestions(tokens);
		validateInput(value);
	}

	function tokenize(value: string) {
		let words = value.split(/(\s+)/).filter((word) => word.trim().length > 0);

		tokens = [];

		words.forEach((token) => {
			if (properties.includes(token)) {
				tokens.push({
					type: 'property',
					value: token
				});
			}

			if (operators.includes(token)) {
				tokens.push({
					type: 'operator',
					value: token
				});
			}

			if (logicals.includes(token)) {
				tokens.push({
					type: 'logical',
					value: token
				});
			}

			if (!properties.includes(token) && !operators.includes(token) && !logicals.includes(token)) {
				tokens.push({
					type: 'value',
					value: token
				});
			}
		});
	}

	function validateInput(input: string): void {
		let lastToken = tokens[tokens.length - 1];

		if (!lastToken) {
			invalidInput = true;
			invalidInputMessage = 'Query cannot be empty';
			return;
		}

		let prevToken = tokens[tokens.length - 2];
		let prevPrevToken = tokens[tokens.length - 3];

		if (!prevToken && lastToken.type !== 'property') {
			// If there is no previous token and last token is not a property, validation fails
			invalidInput = true;
			invalidInputMessage = `Expected a property at position ${
				input.lastIndexOf(lastToken.value) + lastToken.value.length
			}`;
		} else if (prevToken && prevToken.type === 'operator' && lastToken.type !== 'value') {
			// If previous token is an operator and last token is not a value, validation fails
			invalidInput = true;
			invalidInputMessage = `Expected a value after operator at position ${
				input.lastIndexOf(prevToken.value) + prevToken.value.length
			}`;
		} else if (
			lastToken.type === 'property' &&
			((prevToken && prevToken.type !== 'operator') || (!prevToken && tokens.length > 1))
		) {
			// If last token is a property and previous token is not an operator or there is no previous token and there are more than one tokens, validation fails
			invalidInput = true;
			invalidInputMessage = `Expected an operator after property at position ${
				input.lastIndexOf(lastToken.value) + lastToken.value.length
			}`;
		} else if (prevToken && prevToken.type === 'value' && lastToken.type !== 'logical') {
			// If previous token is a value and last token is not a logical operator, validation fails
			invalidInput = true;
			invalidInputMessage = `Expected a logical operator after value at position ${
				input.lastIndexOf(prevToken.value) + prevToken.value.length
			}`;
		} else if (lastToken.type === 'logical' && prevToken && prevToken.type !== 'value') {
			// If last token is a logical operator and previous token is not a value, validation fails
			invalidInput = true;
			invalidInputMessage = `Expected a value after logical operator at position ${
				input.lastIndexOf(lastToken.value) + lastToken.value.length
			}`;
		} else if (
			prevToken &&
			prevToken.type === 'property' &&
			(lastToken.type === 'property' || lastToken.type === 'value')
		) {
			// If previous token is a property and last token is a property or a value, validation fails
			invalidInput = true;
			invalidInputMessage = `Expected an operator after property at position ${
				input.lastIndexOf(prevToken.value) + prevToken.value.length
			}`;
		} else {
			invalidInput = false;
			invalidInputMessage = '';
		}
	}

	function updateSuggestions(tokens: Token[]) {
		let lastToken = tokens[tokens.length - 1];

		suggestions = [];

		if (!lastToken) {
			suggestions = properties;
			return;
		}

		let prevToken = tokens[tokens.length - 2];

		if (lastToken.type === 'property') {
			suggestions = operators;
		} else if (lastToken.type === 'operator') {
			if (prevToken.type === 'property') {
				suggestions = propertySuggestions[prevToken.value];
			}
		} else if (lastToken.type === 'value') {
			if (prevToken && prevToken.type === 'operator') {
				suggestions = logicals;
			}

			let matchingProperties = properties.filter((property) => property.includes(lastToken.value));

			if (matchingProperties.length > 0) {
				suggestions = matchingProperties;
			}
		} else if (lastToken.type === 'logical') {
			suggestions = properties;
		}
	}
</script>

<div>
	<TextInput
		labelText="Query"
		placeholder="Gib deine Query an..."
		bind:value
		bind:invalid={invalidInput}
		bind:invalidText={invalidInputMessage}
	/>

	<div class="grid-row pt-4">
		<div>
			<p>Tokens</p>
			<pre>{JSON.stringify(tokens, null, 2)}</pre>
		</div>
		<div>
			<p>Suggestions</p>
			<pre>{JSON.stringify(suggestions, null, 2)}</pre>
		</div>
	</div>
</div>

<style>
	.pt-4 {
		padding-top: 1rem;
	}

	.grid-row {
		display: grid;
		grid-template-columns: 1fr 1fr;
	}
</style>
