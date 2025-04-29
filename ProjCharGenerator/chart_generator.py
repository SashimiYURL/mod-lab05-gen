import os
import matplotlib.pyplot as plt
from typing import List, Tuple, Optional


class FrequencyCreator:
    def __init__(self):
        self._max_items = 100
        self._figsize = (15, 8)
        self._bar_width = 0.45
        self._dpi = 300
        self._colors = {
            'expected': 'purple',
            'actual': 'orange'
        }

    def read_frequency_data(self, filepath: str) -> List[Tuple[str, float, float]]:
        data = []
        
        if not os.path.exists(filepath):
            print(f"Файл не найден: {filepath}")
            return data

        try:
            with open(filepath, 'r', encoding='utf-8') as file:
                for line in file:
                    if len(data) >= self._max_items:
                        break
                    
                    line = line.strip()
                    if not line:
                        continue
                    
                    parts = line.split()
                    if len(parts) < 3:
                        continue
                    
                    try:
                        label = parts[0]
                        actual = float(parts[1].replace(',', '.'))
                        expected = float(parts[2].replace(',', '.'))
                        data.append((label, expected, actual))
                    except (ValueError, IndexError) as e:
                        print(f"Ошибка обработки строки: {line}. Ошибка: {e}")
                        continue
        
        except Exception as e:
            print(f"Ошибка при чтении файла {filepath}: {e}")
        
        return data

    def _create_plot(self, 
                    labels: List[str], 
                    expected: List[float], 
                    actual: List[float], 
                    title: str, 
                    save_path: str) -> None:
        if not labels or not expected or not actual:
            print("Нет данных для построения графика")
            return

        x_pos = range(len(labels))
        
        plt.figure(figsize=self._figsize)
        
        plt.bar(
            [x - self._bar_width/2 for x in x_pos],
            expected,
            width=self._bar_width,
            label='Ожидаемая частота',
            color=self._colors['expected']
        )
        
        plt.bar(
            [x + self._bar_width/2 for x in x_pos],
            actual,
            width=self._bar_width,
            label='Текущая частота',
            color=self._colors['actual']
        )
        
        plt.title(title, fontsize=14)
        plt.xlabel('Параметры', fontsize=12)
        plt.ylabel('Частота', fontsize=12)
        plt.xticks(x_pos, labels, rotation=90, fontsize=8)
        plt.legend()
        plt.tight_layout()
        plt.grid(True, axis='y', linestyle='--', alpha=0.7)
        
        try:
            plt.savefig(save_path, dpi=self._dpi, bbox_inches='tight')
            print(f"График успешно сохранен: {save_path}")
        except Exception as e:
            print(f"Ошибка при сохранении графика: {e}")
        finally:
            plt.close()

    def create_frequency_plot(self, 
                            data: List[Tuple[str, float, float]], 
                            title: str, 
                            save_path: str) -> None:
       
        if not data:
            print(f"Нет данных для построения графика: {title}")
            return

        labels, expected, actual = zip(*data)
        self._create_plot(labels, expected, actual, title, save_path)

    def analyze_file(self, data_path: str, plot_path: str, title: str) -> bool:
        if not os.path.exists(data_path):
            print(f"Файл данных не найден: {data_path}")
            return False
        
        data = self.read_frequency_data(data_path)
        if not data:
            print(f"Нет данных в файле: {data_path}")
            return False
        
        self.create_frequency_plot(data, title, plot_path)
        return True

    def analyze_all(self) -> None:
        results_dir = os.path.join(os.path.dirname(__file__), "../Results")
        
        bigrams_data = os.path.join(results_dir, "gen-1-stat.txt")
        bigrams_plot = os.path.join(results_dir, "gen-1.png")
        self.analyze_file(bigrams_data, bigrams_plot, 'Распределение частот для биграмм')
        
        words_data = os.path.join(results_dir, "gen-2-stat.txt")
        words_plot = os.path.join(results_dir, "gen-2.png")
        self.analyze_file(words_data, words_plot, 'Распределение частот для слов')


if __name__ == "__main__":
    try:
        generator = FrequencyCreator()
        generator.analyze_all()
    except Exception as e:
        print(f"Произошла ошибка: {e}")