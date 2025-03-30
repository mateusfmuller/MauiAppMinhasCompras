using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views
{
    public partial class ListaProduto : ContentPage
    {
        
        ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

        
        public ListaProduto()
        {
            InitializeComponent();
            lst_produtos.ItemsSource = lista;
        }

        
        protected async override void OnAppearing()
        {
            try
            {
                lista.Clear();
                List<Produto> tmp = await App.Db.GetAll();
                tmp.ForEach(lista.Add);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                Navigation.PushAsync(new Views.NovoProduto());
            }
            catch (Exception ex)
            {
                DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        
        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string novaBusca = e.NewTextValue; 

                lst_produtos.IsRefreshing = true;
                lista.Clear();

                
                List<Produto> tmp = await App.Db.Search(novaBusca);
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
            finally
            {
                lst_produtos.IsRefreshing = false;
            }
        }

        
        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            double soma = lista.Sum(i => i.Total);
            string msg = $"O total é {soma:C}";
            DisplayAlert("Total dos Produtos", msg, "OK");
        }

        
        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                MenuItem selecionado = sender as MenuItem;
                Produto p = selecionado.BindingContext as Produto;

                bool confirm = await DisplayAlert("Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

                if (confirm)
                {
                    await App.Db.Delete(p.Id);
                    lista.Remove(p);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                Produto p = e.SelectedItem as Produto;
                Navigation.PushAsync(new Views.EditarProduto { BindingContext = p });
            }
            catch (Exception ex)
            {
                DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        
        private async void lst_produtos_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lista.Clear();
                List<Produto> tmp = await App.Db.GetAll();
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
            finally
            {
                lst_produtos.IsRefreshing = false;
            }
        }

        private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
        {
                try
                {
                    
                    var produtosPorCategoria = lista
                        .GroupBy(p => p.Categoria)  
                        .Select(g => new
                        {
                            Categoria = g.Key,
                            TotalGasto = g.Sum(p => p.Total)  
                        })
                        .OrderBy(g => g.Categoria)  
                        .ToList();

                    
                    string relatorio = "Relatório de Total por Categoria:\n\n";

                    foreach (var item in produtosPorCategoria)
                    {
                        relatorio += $"{item.Categoria}: {item.TotalGasto:C}\n";
                    }

                    
                    await DisplayAlert("Relatório de Categorias", relatorio, "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ops", ex.Message, "OK");
                }
            }

        }
    }

