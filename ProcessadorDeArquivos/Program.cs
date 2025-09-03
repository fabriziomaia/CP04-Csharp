// Program.cs
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Representa o resultado do processamento de um arquivo.
/// </summary>
/// <param name="FileName">O nome do arquivo.</param>
/// <param name="LineCount">A contagem total de linhas.</param>
/// <param name="WordCount">A contagem total de palavras.</param>
/// <param name="ErrorMessage">Uma mensagem de erro, se o processamento falhar.</param>
public record FileReport(string FileName, int LineCount, int WordCount, string? ErrorMessage = null)
{
    /// <summary>
    /// Indica se ocorreu um erro durante o processamento do arquivo.
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
}

public class Program
{
    /// <summary>
    /// Ponto de entrada principal da aplicação.
    /// </summary>
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=== Processador Assíncrono de Arquivos de Texto ===");
        Console.WriteLine("Informe o caminho de um diretório contendo arquivos .txt:");

        string? directoryPath = Console.ReadLine();

        // Valida o caminho do diretório fornecido pelo usuário
        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Caminho inválido ou diretório não encontrado. Pressione qualquer tecla para sair.");
            Console.ResetColor();
            Console.ReadKey();
            return;
        }

        try
        {
            // Busca todos os arquivos .txt no diretório
            string[] files = Directory.GetFiles(directoryPath, "*.txt");

            if (files.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Nenhum arquivo .txt foi encontrado no diretório especificado.");
                Console.ResetColor();
                return;
            }

            // Lista os arquivos encontrados na tela
            Console.WriteLine($"\n{files.Length} arquivo(s) .txt encontrado(s):");
            foreach (var file in files)
            {
                Console.WriteLine($"- {Path.GetFileName(file)}");
            }
            Console.WriteLine("\nIniciando processamento assíncrono...");

            // Cria uma tarefa de processamento para cada arquivo e as executa em paralelo
            var processingTasks = files.Select(ProcessFileAsync).ToList();
            
            // Aguarda a conclusão de todas as tarefas
            var results = await Task.WhenAll(processingTasks);

            // Gera o relatório consolidado
            await GenerateReportAsync(results);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nProcessamento concluído com sucesso!");
            Console.ResetColor();
            
            string reportPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "export", "relatorio.txt"));
            Console.WriteLine($"Relatório gerado em: {reportPath}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nOcorreu um erro inesperado: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine("\nPressione qualquer tecla para sair.");
        Console.ReadKey();
    }

    /// <summary>
    /// Processa um único arquivo de texto de forma assíncrona para contar linhas e palavras.
    /// </summary>
    /// <param name="filePath">O caminho completo do arquivo a ser processado.</param>
    /// <returns>Um objeto FileReport com os resultados da contagem.</returns>
    public static async Task<FileReport> ProcessFileAsync(string filePath)
    {
        string fileName = Path.GetFileName(filePath);
        Console.WriteLine($"Processando: {fileName} ..."); // Exibe o status de início

        try
        {
            // Lê todo o conteúdo do arquivo de forma assíncrona
            string[] lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
            
            // Calcula a quantidade de linhas e palavras
            int lineCount = lines.Length;
            int wordCount = lines.Sum(line => 
                line.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length);
            
            var report = new FileReport(fileName, lineCount, wordCount);

            // Exibe o resultado individual na tela, mantendo o console responsivo
            Console.WriteLine($"Concluído : {fileName} - {lineCount} linha(s), {wordCount} palavra(s).");
            return report;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Erro ao processar o arquivo {fileName}: {ex.Message}");
            Console.ResetColor();
            // Retorna um relatório de erro se algo falhar
            return new FileReport(fileName, 0, 0, $"Erro: {ex.Message}");
        }
    }

    /// <summary>
    /// Gera o arquivo de relatório consolidado com os resultados do processamento.
    /// </summary>
    /// <param name="reports">Uma coleção de objetos FileReport.</param>
    public static async Task GenerateReportAsync(FileReport[] reports)
    {
        // Define o caminho do diretório de exportação e o cria se não existir
        string exportDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "export");
        Directory.CreateDirectory(exportDir);

        string reportPath = Path.Combine(exportDir, "relatorio.txt"); // Define o caminho do relatório
        
        // Formata cada linha do relatório de acordo com a especificação
        var reportLines = reports
            .OrderBy(r => r.FileName) // Opcional: ordena os resultados pelo nome do arquivo
            .Select(r => r.HasError 
                ? $"{r.FileName} - Erro ao processar."
                : $"{r.FileName} - {r.LineCount} linhas - {r.WordCount} palavras")
            .ToArray();

        // Escreve todas as linhas no arquivo de relatório de forma assíncrona
        await File.WriteAllLinesAsync(reportPath, reportLines, Encoding.UTF8);
    }
}