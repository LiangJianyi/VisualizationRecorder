#include "pch.h"
#include "MainPage.h"
#include "MainPage.g.cpp"
#include <string>

using namespace winrt;
using namespace Windows::UI::Xaml;
using namespace Windows::Foundation;

namespace winrt::TxtRecordGenerator::implementation
{
	MainPage::MainPage() {
		InitializeComponent();
	}
}


void winrt::TxtRecordGenerator::implementation::MainPage::GeneratorButton_Click(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e) {
	IReference<DateTime> date = BeginDatePicker().Date();
	if (date != nullptr) {
		DateTime dt = date.Value();
		auto timeSiceEpoch = dt.time_since_epoch();
		OutputDebugStringA(std::to_string(timeSiceEpoch.count()).c_str());
	}
	else {
		OutputDebugStringA("You not pick a date.");
	}
}
