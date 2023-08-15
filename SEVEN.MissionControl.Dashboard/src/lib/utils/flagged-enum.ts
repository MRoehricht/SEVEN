export class FlaggedEnum<T extends number> {
	private labels: Record<T, string>;
	private values: T[];

	constructor(labels: Record<T, string>) {
		this.labels = labels;
		this.values = Object.keys(labels)
			.map(Number)
			.filter((value) => !isNaN(value)) as T[];
	}

	getAllValuesWithLabels(): [number, string][] {
		return this.values.map((value) => [value, this.labels[value]]);
	}

	getAllValuesAsIdTextObjects(): { id: string; text: string }[] {
		return this.values.map((id) => ({ id: id.toString(), text: this.labels[id] }));
	}

	getValuesAsStrings(value: number): string[] {
		return this.values
			.filter((enumValue) => (value & enumValue) !== 0)
			.map((enumValue) => enumValue.toString());
	}

	getValueFromIds(ids: string[]): number {
		return ids.reduce((acc, id) => acc | Number(id), 0);
	}

	getLabelsFromIds(ids: string[]): string[] {
		return ids.map((id) => this.labels[Number(id) as T]);
	}

	getLabelFromValue(value: number): string {
		return this.values
			.filter((enumValue) => (value & enumValue) !== 0)
			.map((enumValue) => this.labels[enumValue])
			.join(', ');
	}
}
