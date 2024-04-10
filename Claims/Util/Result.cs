public readonly struct Result<T, E> {
    private readonly bool _success;
    public readonly T Value;
    public readonly E Error;

    private Result(T v, E e, bool success)
    {
        Value = v;
        Error = e;
        _success = success;
    }

    public bool IsOk => _success;

    public static Result<T, E> Ok(T v)
    {
        return new Result<T, E>(v, default(E), true);
    }

    public static Result<T, E> Err(E e)
    {
        return new Result<T, E>(default(T), e, false);
    }

    public static implicit operator Result<T, E>(T v) => new(v, default(E), true);
    public static implicit operator Result<T, E>(E e) => new(default(T), e, false);

    public async Task<R> Match<R>(
        Func<T, Task<R>> success,
        Func<E, Task<R>> failure) =>
        _success ? await success(Value) : await failure(Error);
}