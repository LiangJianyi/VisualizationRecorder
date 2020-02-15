#pragma once

#include "MainPage.g.h"

namespace winrt::TxtRecordGenerator::implementation
{
    struct MainPage : MainPageT<MainPage>
    {
        MainPage();
		void GeneratorButton_Click(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
	};
}

namespace winrt::TxtRecordGenerator::factory_implementation
{
    struct MainPage : MainPageT<MainPage, implementation::MainPage>
    {
    };
}
