using System;
using System.IO;
using System.Text;
using Mocassin.Framework.Events;

namespace Mocassin.Framework.Exceptions
{
    /// <summary>
    ///     An event driven <see cref="TextWriter"/> that supports spying on the console output stream
    /// </summary>
    public class ConsoleOutputSpy : TextWriter
    {
        /// <summary>
        ///     Get or set a backup of the original console <see cref="TextWriter"/>
        /// </summary>
        private TextWriter ConsoleOutBackup { get; set; }

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}"/> for string write events
        /// </summary>
        private ReactiveEvent<string> StringWriteEvent { get; } = new ReactiveEvent<string>();

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}"/> for error write events
        /// </summary>
        private ReactiveEvent<Exception> ErrorWriteEvent { get; } = new ReactiveEvent<Exception>();

        /// <inheritdoc />
        public override Encoding Encoding { get; }

        /// <summary>
        ///     Get an <see cref="IObservable{T}"/> that notifies about string write events
        /// </summary>
        public IObservable<string> StringWriteNotifications => StringWriteEvent.AsObservable();

        /// <summary>
        ///     Get an <see cref="IObservable{T}"/> that notifies about string write events
        /// </summary>
        public IObservable<Exception> ErrorWriteNotifications => ErrorWriteEvent.AsObservable();

        /// <summary>
        ///     Get or set a boolean flag if the console output should be suppressed
        /// </summary>
        public bool IsSuppressingConsole { get; set; }

        /// <summary>
        ///     Get a boolean flag if the spy is attached to the console
        /// </summary>
        public bool IsAttached => ConsoleOutBackup != null;

        /// <summary>
        ///     Creates a new <see cref="ConsoleOutputSpy"/> using the provided <see cref="Encoding"/>
        /// </summary>
        /// <param name="encoding"></param>
        public ConsoleOutputSpy(Encoding encoding)
        {
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        /// <summary>
        ///     Creates a <see cref="ConsoleOutputSpy"/> that uses UTF16 encoding
        /// </summary>
        public ConsoleOutputSpy() : this(Encoding.Unicode)
        {
        }

        /// <summary>
        ///     Attaches to the <see cref="Console"/>
        /// </summary>
        public void Attach()
        {
            if (IsAttached) return;
            ConsoleOutBackup = Console.Out;
            Console.SetOut(this);
        }

        /// <summary>
        ///     Detaches from the <see cref="Console"/> and restores the original output behavior
        /// </summary>
        public void Detach()
        {
            if (IsAttached) Console.SetOut(ConsoleOutBackup);
            StringWriteEvent.OnCompleted();
            ErrorWriteEvent.OnCompleted();
            ConsoleOutBackup = null;
        }

        /// <inheritdoc />
        public override void Write(char value)
        {
            if (IsSuppressingConsole) return;
            ConsoleOutBackup.Write(value);
        }

        /// <inheritdoc />
        public override void Write(string value)
        {
            StringWriteEvent.OnNext(value);
            if (IsSuppressingConsole) return;
            ConsoleOutBackup.Write(value);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            Detach();
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override void Write(object value)
        {
            if (value is Exception exception) ErrorWriteEvent.OnNext(exception);
            if (IsSuppressingConsole) return;
            ConsoleOutBackup.Write(value);
        }

        /// <inheritdoc />
        public override void WriteLine(object value)
        {
            if (value is Exception exception) ErrorWriteEvent.OnNext(exception);
            if (IsSuppressingConsole) return;
            ConsoleOutBackup.WriteLine(value);
        }

        /// <inheritdoc />
        public override void WriteLine(string value)
        {
            StringWriteEvent.OnNext(value);
            if (IsSuppressingConsole) return;
            ConsoleOutBackup.WriteLine(value);
        }
    }
}