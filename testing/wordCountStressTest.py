import matplotlib.pyplot as plt

wordCount = [10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 200, 300, 400, 500, 1000]
timeTaken = [3.13, 4.15, 3.88, 6.45, 5.86, 5.75, 5.83, 9.24, 10.80, 11.38, 22.52, 28.79, 40.63, 54.48, 112.50]

plt.figure(figsize=(10, 6))
plt.plot(wordCount, timeTaken, marker='o', color='b', linestyle='-')
plt.title('Time Taken to Generate Voice vs Word Count')
plt.xlabel('Word Count')
plt.ylabel('Time Taken (seconds)')
plt.grid(True)
plt.tight_layout()

note = "For this test, the 10 word phrase 'The sun sets, painting the sky in hues of orange.' was duplicated for increasing the word count."
plt.text(500, 100, note, color='r', fontsize=10, ha='center')
plt.show()