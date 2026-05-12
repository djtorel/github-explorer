import type { ApiError } from "./types.js";

export type Result<T> =
	| { readonly ok: true; readonly value: T }
	| { readonly ok: false; readonly error: ApiError };

export function ok<T>(value: T): Result<T> {
	return { ok: true, value };
}

export function err(error: ApiError): Result<never> {
	return { ok: false, error };
}

export function map<T, U>(result: Result<T>, fn: (value: T) => U): Result<U> {
	return result.ok ? ok(fn(result.value)) : result;
}

export function mapError(
	result: Result<unknown>,
	fn: (error: ApiError) => ApiError,
): Result<unknown> {
	return result.ok ? result : err(fn(result.error));
}
