#include "pch.h"
#include "MainPage.h"
#include "MainPage.g.cpp"
#include "../../SundryUtilty/Cpp code files/JanyeeDateTime/DateTime.h"
#include "../../SundryUtilty/Cpp code files/JanyeeUtilty/Utilty.h"

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
	IReference<DateTime> beginDateRef = BeginDatePicker().Date();
	IReference<DateTime> endDateRef = EndDatePicker().Date();
	if (beginDateRef != nullptr && endDateRef != nullptr) {
		const time_t& bt = beginDateRef.Value().time_since_epoch().count();
		const time_t& et = endDateRef.Value().time_since_epoch().count();
		const Janyee::DateTime& beginDateTime = Janyee::DateTime(bt);
		const Janyee::DateTime& endDateTime = Janyee::DateTime(et);
		const uint32_t ENTRIES_COUNT = 100;
		for (uint32_t i = 0; i < ENTRIES_COUNT; i++) {
			const auto& dateTuple = Janyee::Utilty::RandomDate(
				std::make_tuple(beginDateTime.GetYear(), endDateTime.GetYear()),
				std::make_tuple(beginDateTime.GetMonth(), endDateTime.GetMonth()),
				std::make_tuple(beginDateTime.GetDayOfMonth(), endDateTime.GetDayOfMonth())
			);
			const Janyee::DateTime& date = Janyee::DateTime(std::get<0>(dateTuple), std::get<1>(dateTuple), std::get<2>(dateTuple));
			OutputDebugStringA(date.ToString().c_str());
		}
	}
	else {
		OutputDebugStringA("You not pick a date.");
	}
}
