<script lang="ts">
	import { ClickableTile, Popover, TextArea, TextInput } from 'carbon-components-svelte';

	type Token = {
		type: 'property' | 'operator' | 'logical' | 'value';
		value: string;
		index: number;
	};

	type ValidationError = {
		message: string;
		indexStart: number;
		indexEnd: number;
	};

	let properties = ['MeasurementType', 'ProbeId'];
	let propertySuggestions: Record<string, string[]> = {
		MeasurementType: ['64', '128'],
		ProbeId: ['123', '456']
	};

	let operators = ['==', '!=', '>', '<', '>=', '<='];
	let logicals = ['AND', 'OR', 'NOT'];

	let tokens: Token[] = [];
	let suggestions: string[] = [];
	let value: string = '';
	let invalidInput: boolean = false;
	let invalidInputMessage: string = '';
	let validationErrors: ValidationError[] = [];

	$: {
		tokenize(value);
		validateTokens(value);
		updateSuggestions(tokens);

		if (validationErrors.length == 1) {
			invalidInput = true;
			invalidInputMessage = validationErrors[0].message;
		} else if (validationErrors.length > 0) {
			invalidInput = true;
			invalidInputMessage =
				'More than one error occurred. Please check the error list for more information.';
		} else {
			invalidInput = false;
			invalidInputMessage = '';
		}
	}

	function tokenize(value: string) {
		let words = value.split(/(\s+)/).filter((word) => word.trim().length > 0);

		tokens = [];
		let currentIndex = 0;

		words.forEach((token) => {
			const index = value.indexOf(token, currentIndex);

			if (properties.includes(token)) {
				tokens.push({
					type: 'property',
					value: token,
					index
				});
			}

			if (operators.includes(token)) {
				tokens.push({
					type: 'operator',
					value: token,
					index
				});
			}

			if (logicals.includes(token)) {
				tokens.push({
					type: 'logical',
					value: token,
					index
				});
			}

			if (!properties.includes(token) && !operators.includes(token) && !logicals.includes(token)) {
				tokens.push({
					type: 'value',
					value: token,
					index
				});
			}

			currentIndex = index + token.length;
		});
	}

	function validateTokens(input: string) {
		validationErrors = [];

		if (input.length === 0) {
			validationErrors.push({
				message: 'Expected a non empty string',
				indexStart: 0,
				indexEnd: 0
			});
		}

		for (let i = 0; i < tokens.length; i++) {
			const currentToken = tokens[i];
			const prevToken = tokens[i - 1];

			if (i === 0 && currentToken.type !== 'property') {
				validationErrors.push({
					message: `Expected a property at position ${currentToken.index}`,
					indexStart: currentToken.index,
					indexEnd: currentToken.index + currentToken.value.length
				});
			} else if (prevToken && prevToken.type === 'property' && currentToken.type !== 'operator') {
				validationErrors.push({
					message: `Expected an operator after property at position ${
						currentToken.index + currentToken.value.length
					}`,
					indexStart: currentToken.index,
					indexEnd: currentToken.index + currentToken.value.length
				});
			} else if (prevToken && prevToken.type === 'operator' && currentToken.type !== 'value') {
				validationErrors.push({
					message: `Expected a value after operator at position ${
						currentToken.index + currentToken.value.length
					}`,
					indexStart: currentToken.index,
					indexEnd: currentToken.index + currentToken.value.length
				});
			} else if (prevToken && prevToken.type === 'value' && currentToken.type !== 'logical') {
				validationErrors.push({
					message: `Expected a logical operator after value at position ${
						currentToken.index + currentToken.value.length
					}`,
					indexStart: currentToken.index,
					indexEnd: currentToken.index + currentToken.value.length
				});
			} else if (prevToken && prevToken.type === 'logical' && currentToken.type !== 'property') {
				validationErrors.push({
					message: `Expected a property after logical operator at position ${
						currentToken.index + currentToken.value.length
					}`,
					indexStart: currentToken.index,
					indexEnd: currentToken.index + currentToken.value.length
				});
			}
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

<div style:position="relative">
	<TextInput
		inline
		labelText="DQL Query"
		placeholder="Enter a DQL query"
		bind:value
		bind:invalid={invalidInput}
		bind:invalidText={invalidInputMessage}
	/>
	<!-- <Popover open={suggestions.length > 0} align="bottom-left">
		{#each suggestions as suggestion}
			<ClickableTile>
				<div class="flex-column">
					<span>{suggestion}</span>
				</div>
			</ClickableTile>
		{/each}
	</Popover> -->
	<div class="grid-row pt-4">
		<div>
			<p>Tokens</p>
			<pre>{JSON.stringify(tokens, null, 2)}</pre>
		</div>
		<div>
			<p>Suggestions</p>
			<pre>{JSON.stringify(suggestions, null, 2)}</pre>
		</div>
		<div>
			<p>Errors</p>
			<pre>{JSON.stringify(validationErrors, null, 2)}</pre>
		</div>
	</div>
</div>

<style>
	.pt-4 {
		padding-top: 1rem;
	}

	.grid-row {
		display: grid;
		grid-template-columns: 1fr 1fr 1fr;
	}

	.flex-column {
		display: flex;
		flex-direction: column;
	}
</style>
