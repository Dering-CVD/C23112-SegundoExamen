using GestorDeRestaurante.Dominio.Entidades;

namespace GestorDeRestaurante.Mobile.Views;

public partial class DetalleMenu : ContentPage
{
    public DetalleMenu(Platillo platillo)
    {
        InitializeComponent();
        BindingContext = platillo;
    }
}