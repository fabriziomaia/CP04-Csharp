# Processador Assíncrono de Arquivos de Texto

## Visão Geral do Projeto

Este projeto é uma aplicação de console desenvolvida em C# com .NET 8, criada como parte do desafio de programação do curso. A aplicação tem como objetivo processar múltiplos arquivos de texto (`.txt`) de forma assíncrona e paralela para realizar a contagem de linhas e palavras em cada um. Ao final, um relatório consolidado é gerado em disco.

O principal desafio técnico foi garantir que a aplicação permanecesse responsiva durante o processamento de um grande volume de arquivos, utilizando `async/await` para gerenciar as operações de I/O (leitura e escrita de arquivos) sem bloquear a thread principal.

### Funcionalidades Implementadas

* **Entrada de Diretório**: O usuário é solicitado a fornecer o caminho de uma pasta contendo os arquivos de texto a serem processados.
* **Descoberta de Arquivos**: A aplicação busca e lista todos os arquivos com a extensão `.txt` no diretório especificado.
* **Processamento Assíncrono**: Cada arquivo é processado em uma tarefa separada, permitindo a execução paralela e otimizando o tempo total de processamento.
* **Contagem de Linhas e Palavras**: Para cada arquivo, o programa lê seu conteúdo e calcula o número total de linhas e palavras.
* **Feedback em Tempo Real**: O console exibe mensagens de progresso, informando qual arquivo está sendo processado e o resultado individual ao concluir, garantindo que a interface não "congele".
* **Geração de Relatório**: Os resultados de todos os arquivos são consolidados e salvos em um arquivo `relatorio.txt`, localizado em uma pasta `export` no diretório da aplicação.

### Tecnologias Utilizadas

* **Linguagem**: C# 12
* **Plataforma**: .NET 8
* **Tipo de Projeto**: Console Application
* **Principais Recursos**:
    * Programação Assíncrona (`async`/`await`, `Task.WhenAll`)
    * Manipulação de Arquivos e Diretórios (`System.IO`)
    * LINQ para manipulação de coleções de dados

---

### Integrantes do Grupo

* RM551869 - Fabrizio Maia
* RM551684 - Victor Asfur
* RM550390 - Vitor Shimizu