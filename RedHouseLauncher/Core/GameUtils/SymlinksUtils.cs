using System.IO;

namespace RedHouseLauncher.Core.GameUtils
{
    internal static class SymlinksUtils
    {
        #region Рекурсивное удаление символических ссылок в папке

        internal static void DeleteSymlinksRecursive(string directory)
        {
            #region Удаление этой директории, если символическая

            if (IsSymbolicDirectory(directory))
            {
                Directory.Delete(directory, true);
                return;
            }

            #endregion

            #region Проверка директорий

            string[] dirs = Directory.GetDirectories(directory);

            foreach (string directoryToCheck in dirs)
            {
                DeleteSymlinksRecursive(directoryToCheck);
            }

            #endregion

            #region Проверка файлов

            string[] files = Directory.GetFiles(directory);

            foreach (string file in files)
            {
                if (IsSymbolicFile(file))
                {
                    File.Delete(file);
                }
            }

            #endregion
        }

        #endregion

        #region Проверки

        private static bool IsSymbolicFile(string path)
        {
            FileInfo pathInfo = new(path);
            return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        private static bool IsSymbolicDirectory(string path)
        {
            DirectoryInfo pathInfo = new(path);
            return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        #endregion
    }
}
