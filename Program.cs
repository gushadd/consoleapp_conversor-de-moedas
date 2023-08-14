using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace ConversorDeMoedas;

class Program
{
    static async Task Main(string[] args)
    {
        bool realizarNovaAcao = true;
        do
        {
            Console.Clear();
            int opcao = PegaOpcao();

            switch (opcao)
            {
                case 1:
                    Console.Clear();
                    MostraListaDeMoedas();
                    break;
                case 2:
                    Console.Clear();
                    Console.WriteLine("Digite as moedas para conversão da seguinte forma: USD-BRL");
                    Console.WriteLine("Caso queira mais de uma conversão, separe-as por vírgula (BRL-USD,BRL-EUR)");
                    Console.Write("Digite: ");
                    string moedas = Console.ReadLine();

                    string urlApi = $"https://economia.awesomeapi.com.br/last/{moedas}";

                    using (HttpClient httpClient = new HttpClient())
                    {
                        try
                        {
                            // Envia uma requisição GET para a URL da API
                            HttpResponseMessage resposta = await httpClient.GetAsync(urlApi);

                            if (resposta.IsSuccessStatusCode)
                            {
                                // Lê a resposta como uma string JSON
                                string jsonResposta = await resposta.Content.ReadAsStringAsync();
                                // Converte a string JSON em um objeto JSON (JObject)
                                JObject json = JObject.Parse(jsonResposta);

                                foreach (var moeda in json)
                                {
                                    string nomeDaMoeda = moeda.Value["name"].ToString();
                                    string precoCompra = moeda.Value["bid"].ToString();
                                    string precoVenda = moeda.Value["ask"].ToString();

                                    Console.WriteLine($"\nConversão: {nomeDaMoeda}");
                                    Console.WriteLine($"Valor de compra: {precoCompra}");
                                    Console.WriteLine($"Valor de venda: {precoVenda}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Erro: entrada errada de dados!");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Excecão: {ex.Message}");
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Opção inválida!");
                    Thread.Sleep(2000);
                    continue;
            }

            string simOuNao;
            do
            {
                Console.Write("\nDeseja realizar nova ação (s/n)? ");
                simOuNao = Console.ReadLine();
                Console.Clear();
            } while (!OpcaoNovaAcaoCorreta(simOuNao));

            realizarNovaAcao = Char.ToUpper(Convert.ToChar(simOuNao)) == 'S';

        } while (realizarNovaAcao);
    }

    static void MostraListaDeMoedas()
    {
        string xmlMoedasCaminho = "moedas.xml";

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlMoedasCaminho);
        XmlNodeList moedasList = xmlDoc.DocumentElement.ChildNodes;

        foreach (XmlNode moedaNode in moedasList)
        {
            string codigo = moedaNode.Name;
            string nome = moedaNode.InnerText;
            int numeroDeHifens = 15 - codigo.Length;
            string hifens = new String('-', numeroDeHifens);

            Console.WriteLine($"{codigo} {hifens} {nome}");
        }
    }

    static int PegaOpcao()
    {
        Console.WriteLine("1 - Ver lista de moedas");
        Console.WriteLine("2 - Converter moedas");
        Console.Write("Selecione uma opção: ");
        string resposta = Console.ReadLine();

        while (string.IsNullOrEmpty(resposta) || resposta.Length != 1 || !char.IsNumber(resposta[0]))
        {
            Console.Write("\nPor favor, digite um número: ");
            resposta = Console.ReadLine();
        }

        int opcao = Convert.ToInt32(resposta);

        while (opcao != 1 && opcao != 2)
        {
            Console.Write("\nDigite '1' ou '2': ");
            opcao = Convert.ToInt32(Console.ReadLine());
        }

        return opcao;
    }
    static bool OpcaoNovaAcaoCorreta(string simOuNao)
    {
        if (string.IsNullOrEmpty(simOuNao) || simOuNao.Length != 1 || !char.IsLetter(simOuNao[0]))
        {
            Console.WriteLine("\nPor favor, digite um caractere.");
            return false;
        }

        char maiuscula = char.ToUpper(simOuNao[0]);

        if (maiuscula != 'S' && maiuscula != 'N')
        {
            Console.WriteLine("\nDigite 'S' ou 'N'.");
            return false;
        }

        return true;
    }
}
