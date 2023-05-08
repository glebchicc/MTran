using System.Diagnostics;
using System.Runtime.Loader;

namespace lab2
{
    internal class lab5
    {
        public static void Compiler(string kotlinFilePath, string compiledFilePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo { FileName = "D:\\projects\\MTrab\\repo\\MTran\\lab2\\bin\\Debug\\net7.0\\kotlinc\\kotlinc\\bin\\kotlinc.bat", Arguments = $"{kotlinFilePath} -include-runtime -d {compiledFilePath}", RedirectStandardOutput = true, UseShellExecute = false };

            Process process = new Process { StartInfo = startInfo };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode == 0) // если код возврата равен 0 (успешное завершение), то выводим результат
            {
                Console.WriteLine("Compilation successful!\n");

                string command = "java -jar D:\\projects\\MTrab\\repo\\MTran\\lab2\\bin\\Debug\\net7.0\\file.jar";
                ProcessStartInfo startInfo2 = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                using (Process process_ = new Process())
                {
                    process_.StartInfo = startInfo2;
                    process_.Start();

                    // Получаем вывод командной строки
                    string output_ = process_.StandardOutput.ReadToEnd();
                    string error = process_.StandardError.ReadToEnd();

                    // Выводим вывод командной строки в консоль
                    Console.WriteLine(output_);

                    // Выводим ошибки командной строки в консоль
                    Console.WriteLine(error);
                }
            }
            else // если код возврата отличается от 0 (неуспешное завершение), то выводим ошибку
            {
                Console.WriteLine($"Compilation failed with exit code {process.ExitCode}");
                Console.WriteLine(output);
            }
        }
    }
}
