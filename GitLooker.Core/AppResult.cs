namespace GitLooker.Core
{
    public class AppResult<T>
    {
        private List<T> value = new List<T>();
        private List<Exception> error = new List<Exception>();
        public bool IsSuccess { get; }
        public IEnumerable<T> Value => value;
        public IEnumerable<Exception> Error => error;
        public object SpecialValue { get; set; }

        public AppResult(T value)
        {
            if (value == null)
                error.Add(new ArgumentNullException(nameof(value)));
            else
            {
                IsSuccess = true;
                this.value.Add(value);
            }
        }

        public AppResult(Exception exception)
            => error.Add((exception ?? new ArgumentNullException(nameof(exception))));

        public void Add(AppResult<T> result)
        {
            if (IsSuccess && (result?.IsSuccess ?? false))
                value.AddRange(result?.Value);
            else
                error.AddRange(result?.Error);
        }
    }
}
