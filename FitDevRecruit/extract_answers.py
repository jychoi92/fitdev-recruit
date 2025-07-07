import json
import os

def extract_answers():
    # JSON 파일 읽기
    with open('questions.json', 'r', encoding='utf-8') as f:
        questions = json.load(f)
    
    # 정답 텍스트 생성
    answer_text = "=== 솔로몬텍 인재 채용 평가 정답표 ===\n\n"
    
    for question in questions:
        q_id = question['Id']
        q_text = question['Text']
        q_type = question['Type']
        q_category = question['Category']
        q_level = question['TargetLevel']
        q_team = question['Team']
        
        answer_text += f"[문항 {q_id}] {q_text}\n"
        answer_text += f"유형: {q_type} | 카테고리: {q_category} | 레벨: {q_level} | 팀: {q_team}\n"
        
        if q_type == "MultipleChoice":
            choices = question['Choices']
            correct_index = question['CorrectChoiceIndex']
            if correct_index is not None and correct_index < len(choices):
                answer_text += f"정답: {choices[correct_index]} (선택지 {correct_index + 1}번)\n"
            else:
                answer_text += "정답: 정답 정보 없음\n"
        elif q_type == "Personality":
            personality_score = question['PersonalityScore']
            if personality_score is not None:
                answer_text += f"인성 점수: {personality_score}점\n"
            else:
                answer_text += "인성 점수: 점수 정보 없음\n"
        else:  # Subjective
            answer_text += "정답: 주관식 문항 (개인 경험 및 지식 기반 답변)\n"
        
        answer_text += "\n" + "-" * 80 + "\n\n"
    
    # 파일로 저장
    with open('정답표.txt', 'w', encoding='utf-8') as f:
        f.write(answer_text)
    
    print(f"총 {len(questions)}개 문항의 정답이 '정답표.txt' 파일로 저장되었습니다.")

if __name__ == "__main__":
    extract_answers() 