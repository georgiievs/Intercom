namespace Intercom.Application
{
    public class Result<T>
    {
        protected Result() { } // Запрещаем вызов конструктора

        public T? Value { get; protected set; } = default;
        public bool IsSuccess { get; protected set; }
        public string Message { get; protected set; } = string.Empty;

        /// <summary>
        /// Создание успешного результата.
        /// </summary>
        /// <param name="value">Значение полученное в результате выполнения операции</param>
        /// <returns></returns>
        public static Result<T> Success(T value)
        {
            return new Result<T>()
            {
                IsSuccess = true,
                Value = value
            };
        }

        /// <summary>
        /// Создание результата с ошибкой
        /// </summary>
        /// <param name="message">Текст ошибки</param>
        /// <returns></returns>
        public static Result<T> Fail(string message)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}
